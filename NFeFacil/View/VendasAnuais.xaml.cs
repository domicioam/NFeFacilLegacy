﻿using NFeFacil.ItensBD;
using NFeFacil.ModeloXML;
using NFeFacil.ModeloXML.PartesProcesso;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace NFeFacil.View
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class VendasAnuais : Page, IHambuguer
    {
        public VendasAnuais()
        {
            InitializeComponent();
            using (var db = new AplicativoContext())
            {
                AnosDisponiveis = (from dado in db.NotasFiscais
                                   let ano = Convert.ToDateTime(dado.DataEmissao).Year
                                   orderby ano ascending
                                   select ano).Distinct().GerarObs();
                NotasFiscais = (from item in db.NotasFiscais
                                where item.Status >= 4
                                let data = DateTime.Parse(item.DataEmissao)
                                let xml = XElement.Parse(item.XML)
                                let nota = xml.FirstNode.FromXElement<NFe>()
                                group nota by data.Year).ToDictionary(x => x.Key, x => x.ToArray());
            }
            ResultadoMes = new ObservableCollection<TotalPorMes>();
            ResultadoCliente = new ObservableCollection<TotalPorCliente>();

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MainPage.Current.SeAtualizar(Symbol.Calendar, "Vendas");
        }

        public IEnumerable ConteudoMenu => new ObservableCollection<Controles.ItemHambuguer>
        {
            new Controles.ItemHambuguer(Symbol.Calendar, "Meses"),
            new Controles.ItemHambuguer(Symbol.People, "Clientes"),
        };

        public void AtualizarMain(int index) => flipView.SelectedIndex = index;

        private void TelaMudou(object sender, SelectionChangedEventArgs e)
        {
            var index = ((FlipView)sender).SelectedIndex;
            MainPage.Current.AlterarSelectedIndexHamburguer(index);
        }

        Dictionary<int, NFe[]> NotasFiscais;

        ObservableCollection<int> AnosDisponiveis { get; }
        ObservableCollection<TotalPorMes> ResultadoMes { get; }
        ObservableCollection<TotalPorCliente> ResultadoCliente { get; }

        private void AnoEscolhidoMudou(object sender, SelectionChangedEventArgs e)
        {
            var AnoEscolhido = (int)e.AddedItems[0];
            try
            {
                using (var db = new AplicativoContext())
                {
                    var notas = NotasFiscais;

                    ResultadoCliente.Clear();
                    var gruposClientes = from nota in notas[AnoEscolhido]
                                         group nota by nota.Informacoes.destinatário.Documento into item
                                         let total = item.Sum(x => x.Informacoes.total.ICMSTot.VNF)
                                         orderby total descending
                                         select new { Notas = item, Total = total };
                    foreach (var item in gruposClientes)
                    {
                        var det = item.Notas.Last().Informacoes;
                        var atual = new TotalPorCliente
                        {
                            Doc = det.destinatário.Documento,
                            Mun = det.destinatário.Endereco.NomeMunicipio,
                            Nome = det.destinatário.Nome,
                            Quantidade = item.Notas.Sum(x => x.Informacoes.produtos.Sum(prod => prod.Produto.QuantidadeComercializada)),
                            Total = item.Total
                        };
                        ResultadoCliente.Add(atual);
                    }

                    ResultadoMes.Clear();
                    var gruposMeses = from nota in notas[AnoEscolhido]
                                      let data = DateTime.Parse(nota.Informacoes.identificacao.DataHoraEmissão)
                                      orderby data.Month
                                      group new { Nota = nota, Data = data.Month } by data.Month;
                    foreach (var item in gruposMeses)
                    {
                        var primeiro = item.First();
                        var atual = new TotalPorMes
                        {
                            Mês = primeiro.Data.ToString(),
                            Quantidade = item.Sum(det => det.Nota.Informacoes.produtos.Sum(prod => prod.Produto.QuantidadeComercializada)),
                            Total = item.Sum(det => det.Nota.Informacoes.total.ICMSTot.VNF)
                        };
                        ResultadoMes.Add(atual);
                    }
                }
            }
            catch (Exception erro)
            {
                erro.ManipularErro();
            }
        }

        public struct TotalPorCliente
        {
            public double Quantidade { get; set; }
            public double Total { get; set; }
            public string Nome { get; set; }
            public string Doc { get; set; }
            public string Mun { get; set; }
        }

        public sealed class TotalPorMes
        {
            public double Quantidade { get; set; }
            public double Total { get; set; }
            public string Mês { get; set; }
        }
    }
}
