﻿using Microsoft.EntityFrameworkCore;
using NFeFacil.ItensBD;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace NFeFacil.ViewDadosBase.GerenciamentoProdutos
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class GerenciarProdutos : Page
    {
        ObservableCollection<ProdutoDI> Produtos { get; }

        public GerenciarProdutos()
        {
            InitializeComponent();
            using (var repo = new Repositorio.MEGACLASSE())
            {
                Produtos = repo.ObterProdutos().GerarObs();
            }
        }

        private void AdicionarProduto(object sender, RoutedEventArgs e)
        {
            MainPage.Current.Navegar<AdicionarProduto>();
        }

        private void EditarProduto(object sender, RoutedEventArgs e)
        {
            var contexto = ((FrameworkElement)sender).DataContext;
            MainPage.Current.Navegar<AdicionarProduto>((ProdutoDI)contexto);
        }

        async void ControlarEstoque(object sender, RoutedEventArgs e)
        {
            var contexto = ((FrameworkElement)sender).DataContext;
            var produto = (ProdutoDI)contexto;
            using (var repo = new Repositorio.MEGACLASSE())
            {
                var estoque = repo.ObterEstoque(produto.Id);
                if (estoque == null)
                {
                    var caixa = new MessageDialog("Essa é uma operação sem volta, uma vez adicionado ao controle de estoque este produto será permanentemente parte dele. Certeza que você realmente quer isso?", "Atenção");
                    caixa.Commands.Add(new UICommand("Sim", x =>
                    {
                        estoque = new Estoque() { Id = produto.Id };
                        repo.AdicionarEstoque(estoque, Propriedades.DateTimeNow);
                        repo.SalvarComTotalCerteza();
                    }));
                    caixa.Commands.Add(new UICommand("Não"));
                    if ((await caixa.ShowAsync()).Label == "Não") return;
                }
                MainPage.Current.Navegar<ControleEstoque>(estoque);
            }
        }

        private void InativarProduto(object sender, RoutedEventArgs e)
        {
            var contexto = ((FrameworkElement)sender).DataContext;
            var prod = (ProdutoDI)contexto;

            using (var repo = new Repositorio.MEGACLASSE())
            {
                repo.InativarProduto(prod, Propriedades.DateTimeNow);
                Produtos.Remove(prod);
            }
        }
    }
}
