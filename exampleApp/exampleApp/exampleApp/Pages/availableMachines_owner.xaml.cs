﻿using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace exampleApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    
    public partial class availableMachines_owner : ContentPage
    {
        public bool isReady = false;
        public class Machinebind
        {
            public string name { get; set; }
            public int id_machine { get; set; }
            public bool available { get; set; }

        }
        ObservableCollection<Machinebind> list_bind;
        public ObservableCollection<Machinebind> List_bind { get { return list_bind; } }

        public static List<Models.Machine> machines_list;

        MySqlConnection conn;
        public availableMachines_owner()
        {
            InitializeComponent();
            list_bind = new ObservableCollection<Machinebind>();
            InitList();
            isReady = true;
            ConnectDataBase();
        }
        private void ConnectDataBase()
        {
            
            try
            {

                Console.WriteLine("Trying to connect");
                var builder = new MySqlConnectionStringBuilder
                {
                    Server = "gymservernew.mysql.database.azure.com",
                    Database = "gym_schema",
                    UserID = "gymAdmin",
                    Password = "gym1Admin",
                    SslMode = MySqlSslMode.Required,
                };

                conn = new MySqlConnection(builder.ConnectionString);

                conn.Open();
                Console.WriteLine(conn.State.ToString() + Environment.NewLine);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public void InitList()
        {
            foreach (Models.Machine m in machines_list)
            {
                if (m.Available == 1)
                {
                    Machinebind temp = new Machinebind { name = m.Name, id_machine = m.Id_machine, available = true };
                    list_bind.Add(temp);

                }
                else
                {
                    Machinebind temp = new Machinebind { name = m.Name, id_machine = m.Id_machine, available = false };
                    list_bind.Add(temp);
                }
            }
            available_machines.ItemsSource = list_bind;

        }
        public void delete_clicked(Object sender, System.EventArgs e)
        {
            Button thebutton = (Button)sender;
            Machinebind machine = thebutton.BindingContext as Machinebind;
            
            using (MySqlCommand command = conn.CreateCommand())
            {

                command.CommandText = @"DELETE FROM machines WHERE idmachine=@id_machine and name=@name;";
                command.Parameters.AddWithValue("@id_machine", (machine.id_machine));
                command.Parameters.AddWithValue("name",machine.name);
       
                command.ExecuteNonQuery();


            }
            list_bind.Remove(machine);
            available_machines.ItemsSource = list_bind;
        }
        public void addMachineButton_Clicked(Object sender, System.EventArgs e)
        {
            popupAdd.IsVisible = true;


        }
        public void click_button_cancel(Object sender, System.EventArgs e)
        {
            popupAdd.IsVisible = false;

        }
        public async void click_button_save(Object sender, System.EventArgs e)
        {
            string msg;
            int new_id=-1;
            bool isDUplicate = false;
            string machine_name = entry.Text;
            if (machine_name == "" || machine_name==null)
            {
                msg = " you didn't enter a name for the new machine";
                await Application.Current.MainPage.DisplayAlert("Empty Machine Name", msg, "OK");
            }
            else
            {
                using (MySqlCommand command = conn.CreateCommand())
                {
                    command.CommandText = @"SELECT idmachine,name FROM gym_schema.machines WHERE name=@name;";
                    command.Parameters.AddWithValue("@name", machine_name);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())

                        {
                            if (!reader.IsDBNull(0))

                                isDUplicate = true;

                        }
                    }
                }
                if (!isDUplicate)
                {

                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = @"INSERT INTO machines(name) VALUES (@name);";
                        command.Parameters.AddWithValue("@name", machine_name);
                        command.ExecuteNonQuery();
                    }
                    using (MySqlCommand command = conn.CreateCommand())
                    {
                        command.CommandText = @"SELECT idmachine FROM gym_schema.machines WHERE name=@name;";
                        command.Parameters.AddWithValue("@name", machine_name);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())

                            {
                                new_id = reader.GetInt32(0);
                            }
                        }
                    }


                    Machinebind new_machine = new Machinebind { name = machine_name, id_machine = new_id, available = true };
                    list_bind.Add(new_machine);
                    available_machines.ItemsSource = list_bind;
                    popupAdd.IsVisible = false;
                }
                else
                {
                    msg = "there is already a machine with the same name, please try diffrent name";
                    await Application.Current.MainPage.DisplayAlert("duplicate machine name", msg, "OK");
                }
            }

        }

            private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            if (isReady)
            {
                Switch thebutton = (Switch)sender;
                Machinebind machine = thebutton.BindingContext as Machinebind;
                if (machine != null)
                {
                    int new_working;
                    if (machine.available)
                        new_working = 1;
                    else new_working = 0;
                    using (MySqlCommand command = conn.CreateCommand())
                    {

                        command.CommandText = @"UPDATE machines SET working=@new_working WHERE idmachine=@id_machine and name=@name;";
                        command.Parameters.AddWithValue("@id_machine", machine.id_machine);
                        command.Parameters.AddWithValue("@new_working", new_working);
                        command.Parameters.AddWithValue("@name", machine.name);
                        command.ExecuteNonQuery();


                    }
                    machine.available = !machine.available;
                    available_machines.ItemsSource = list_bind;
                }
            }
        }
    }
}