﻿using NFeFacil.ItensBD;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace NFeFacil.ViewDadosBase
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class GerenciarCompradores : Page
    {
        ObservableCollection<ExibicaoComprador> Compradores { get; }

        public GerenciarCompradores()
        {
            InitializeComponent();
            using (var repo = new Repositorio.MEGACLASSE())
            {
                var original = repo.ObterCompradores();
                Compradores = new ObservableCollection<ExibicaoComprador>();
                foreach (var atual in original)
                {
                    Compradores.Add(new ExibicaoComprador()
                    {
                        Root = atual.Item2,
                        NomeEmpresa = atual.Item1
                    });
                }
            }
        }

        private void AdicionarComprador(object sender, RoutedEventArgs e)
        {
            MainPage.Current.Navegar<AdicionarComprador>();
        }

        private void EditarComprador(object sender, RoutedEventArgs e)
        {
            var contexto = ((FrameworkElement)sender).DataContext;
            MainPage.Current.Navegar<AdicionarComprador>(((ExibicaoComprador)contexto).Root);
        }

        private void InativarComprador(object sender, RoutedEventArgs e)
        {
            var contexto = ((FrameworkElement)sender).DataContext;
            var compr = (ExibicaoComprador)contexto;

            using (var repo = new Repositorio.MEGACLASSE())
            {
                repo.InativarComprador(compr.Root, Propriedades.DateTimeNow);
                Compradores.Remove(compr);
            }
        }
    }

    struct ExibicaoComprador
    {
        public Comprador Root { get; set; }
        public string NomeEmpresa { get; set; }
    }
}
