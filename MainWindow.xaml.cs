﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using MySql.Data.MySqlClient;


namespace WpfApp9
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string kapcsolatLeiro = "datasource=127.0.0.1;port=3306;username=root;password=;database=hardver;";
        List<Termek> termekek = new List<Termek>();
        MySql.Data.MySqlClient.MySqlConnection SQLkapcsolat;
        public MainWindow()
        {
            InitializeComponent();
            AdatbazisMegnyitas();
            KategoriakBetoltese();
            GyartokBetoltese();
            TermekekBetolteseListaba();

        }

        private string SzukitoLekerdezesEloallitasa()
        {
            string SQLSzukitettLista = "SELECT * FROM termékek";
            bool feltetel = false;
            if (cbKategoria.SelectedIndex > 0 || cbGyarto.SelectedIndex > 0 || txtTermek.Text != "")
            {
             SQLSzukitettLista += " WHERE ";
            }


            if (cbKategoria.SelectedIndex > 0)
            {
                if (feltetel)
                {
                 SQLSzukitettLista += " AND ";
                }
                SQLSzukitettLista += $"kategória='{cbKategoria.SelectedItem}'";
                feltetel = true;
            }


            if (txtTermek.Text != "")
            {
                if (feltetel)
                {
                 SQLSzukitettLista += " AND ";
                }
                SQLSzukitettLista += $"név LIKE '%{txtTermek.Text}%'";

            }


            if (cbGyarto.SelectedIndex > 0)
            {
                SQLSzukitettLista += $"gyártó='{cbGyarto.SelectedItem}'";
                feltetel = true;
            }

            return (SQLSzukitettLista);
        }

        private void AdatbazisMegnyitas()
        {
            try
            {
                SQLkapcsolat = new MySqlConnection(kapcsolatLeiro);
                SQLkapcsolat.Open();
            }
            catch (Exception)
            {

                MessageBox.Show("Nem tud kapcsolodni");
                this.Close();
            }
        }

        private void AdatbazisLezarasa()
        {
            SQLkapcsolat.Close();
        }

        private void TermekekBetolteseListaba()
        {
            string SQLOsszesTermek = "SELECT * FROM termékek;";
            MySqlCommand SQLparancs = new MySqlCommand(SQLOsszesTermek, SQLkapcsolat);
            MySqlDataReader eredmenyOlvaso = SQLparancs.ExecuteReader();

            while (eredmenyOlvaso.Read())
            {
                Termek uj = new Termek(eredmenyOlvaso.GetString(1),
                    eredmenyOlvaso.GetString(2),
                    eredmenyOlvaso.GetString(3),
                    eredmenyOlvaso.GetInt32(4),
                    eredmenyOlvaso.GetInt32(5));

                termekek.Add(uj);

            }
            eredmenyOlvaso.Close();
            dgTermekek.ItemsSource = termekek;
        }

        private void KategoriakBetoltese()
        {
            string SQLKategoriakRendezve = "SELECT DISTINCT Kategória FROM termékek ORDER BY Kategória;";
            MySqlCommand SQLparancs = new MySqlCommand(SQLKategoriakRendezve, SQLkapcsolat);
            MySqlDataReader eredmenyOlvaso = SQLparancs.ExecuteReader();

            cbKategoria.Items.Add("- Nincs Megadva -");
            while (eredmenyOlvaso.Read())
            {
                cbKategoria.Items.Add(eredmenyOlvaso.GetString(0));
            }
            eredmenyOlvaso.Close();
            cbKategoria.SelectedIndex = 0;
        }

        private void GyartokBetoltese()
        {
            string SQLGyartokRendezve = "SELECT DISTINCT gyártó FROM termékek ORDER BY gyártó;";
            MySqlCommand SQLparancs = new MySqlCommand(SQLGyartokRendezve, SQLkapcsolat);
            MySqlDataReader eredmenyOlvaso = SQLparancs.ExecuteReader();
            cbGyarto.Items.Add("- Nincs Megadva -");
            while (eredmenyOlvaso.Read())
            {
                cbGyarto.Items.Add(eredmenyOlvaso.GetString(0));
            }
            eredmenyOlvaso.Close();
            cbGyarto.SelectedIndex = 0;
        }


        private void btnMentes_Click(object sender, RoutedEventArgs e)
        {
            StreamWriter sw = new StreamWriter("alma.csv");
            foreach (var item in termekek)
            {
                sw.WriteLine(item.ToCSVString());
            }
            sw.Close();
        }

        private void btnSzukit_Click(object sender, RoutedEventArgs e)
        {
            termekek.Clear();
            string SQLSzukitettLista = SzukitoLekerdezesEloallitasa();

            MySqlCommand SQLParancs = new MySqlCommand(SQLSzukitettLista, SQLkapcsolat);
            MySqlDataReader eredmenyOlvaso = SQLParancs.ExecuteReader();

            while (eredmenyOlvaso.Read())
            {
                Termek uj = new Termek(eredmenyOlvaso.GetString("Kategória"),
                    eredmenyOlvaso.GetString("Gyártó"),
                    eredmenyOlvaso.GetString("Név"),
                    eredmenyOlvaso.GetInt32("Ár"),
                    eredmenyOlvaso.GetInt32("Garidő"));
                termekek.Add(uj);
            }
            eredmenyOlvaso.Close();
            dgTermekek.Items.Refresh();
        }
    }
}